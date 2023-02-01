﻿// Copyright 2022 The NATS Authors
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Threading;
using System.Threading.Tasks;
using NATS.Client.Internals;

namespace NATS.Client.JetStream
{
    public abstract class FeatureBaseAsync
    {
        internal JetStream js;
        internal IJetStreamManagementAsync jsm;
        internal string StreamName { get; set; }

        protected FeatureBaseAsync(IConnection connection, FeatureOptions fo)
        {
            if (fo == null)
            {
                js = new JetStream(connection, null);
                jsm = new JetStreamManagementAsync(connection, null);
            }
            else
            {
                js = new JetStream(connection, fo.JSOptions);
                jsm = new JetStreamManagementAsync(connection, fo.JSOptions);
            }
        }

        internal async Task<MessageInfo> _getLastAsync(string subject, CancellationToken cancellationToken)
        {
            try {
                return await jsm.GetLastMessageAsync(StreamName, subject, cancellationToken).ConfigureAwait(false);
            }
            catch (NATSJetStreamException njse) {
                if (njse.ApiErrorCode == JetStreamConstants.JsNoMessageFoundErr) {
                    return null;
                }
                throw;
            }
        }

        internal async Task<MessageInfo> _getBySeqAsync(ulong sequence, CancellationToken cancellationToken)
        {
            try {
                return await jsm.GetMessageAsync(StreamName, sequence, cancellationToken).ConfigureAwait(false);
            }
            catch (NATSJetStreamException njse) {
                if (njse.ApiErrorCode == JetStreamConstants.JsNoMessageFoundErr) {
                    return null;
                }
                throw;
            }
        }
 
        internal void VisitSubject(string subject, DeliverPolicy deliverPolicy, bool headersOnly, bool ordered, Action<Msg> action) {
            PushSubscribeOptions pso = PushSubscribeOptions.Builder()
                .WithOrdered(ordered)
                .WithConfiguration(
                    ConsumerConfiguration.Builder()
                        .WithAckPolicy(AckPolicy.None)
                        .WithDeliverPolicy(deliverPolicy)
                        .WithHeadersOnly(headersOnly)
                        .Build())
                .Build();
            
            IJetStreamPushSyncSubscription sub = js.PushSubscribeSync(subject, pso);
            try
            {
                bool lastTimedOut = false;
                ulong pending = sub.GetConsumerInformation().CalculatedPending;
                while (pending > 0) // no need to loop if nothing pending
                {
                    try
                    {
                        Msg m = sub.NextMessage(js.Timeout);
                        action.Invoke(m);
                        if (--pending == 0)
                        {
                            return;
                        }
                        lastTimedOut = false;
                    }
                    catch (NATSTimeoutException)
                    {
                        if (lastTimedOut)
                        {
                            return; // two timeouts in a row is enough
                        }
                        lastTimedOut = true;
                    }
                }
            }
            finally
            {
                sub.Unsubscribe();
            }
        }
    }
}