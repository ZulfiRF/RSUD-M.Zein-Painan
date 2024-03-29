﻿using System;
using System.Collections.Generic;
using Core.Framework.Messaging.Classes;

namespace Core.Framework.Messaging.Handlers.WebSocket
{
    internal class WebSocketHandler : Handler
    {
        /// <summary>
        /// Handles the request.
        /// </summary>
        /// <param name="context">The user context.</param>
        public override void HandleRequest(Context context)
        {
            if (context.IsSetup)
            {
                context.UserContext.DataFrame.Append(context.Buffer, true);
                if (context.UserContext.DataFrame.Length <= context.MaxFrameSize)
                {
                    switch (context.UserContext.DataFrame.State)
                    {
                        case DataFrame.DataState.Complete:
                            context.UserContext.OnReceive();
                            break;

                        case DataFrame.DataState.Closed:
                            context.UserContext.Send(context.UserContext.DataFrame.CreateInstance(), false, true);
                            break;

                        case DataFrame.DataState.Ping:
                            context.UserContext.DataFrame.State = DataFrame.DataState.Complete;
                            DataFrame dataFrame = context.UserContext.DataFrame.CreateInstance();
                            dataFrame.State = DataFrame.DataState.Pong;
                            List<ArraySegment<byte>> pingData = context.UserContext.DataFrame.AsRaw();
                            foreach (var item in pingData)
                            {
                                dataFrame.Append(item.Array);
                            }
                            context.UserContext.Send(dataFrame);
                            break;

                        case DataFrame.DataState.Pong:
                            context.UserContext.DataFrame.State = DataFrame.DataState.Complete;
                            break;
                    }
                }
                else
                {
                    context.Disconnect(); //Disconnect if over MaxFrameSize
                }
            }
            else
            {
                Authentication.Authenticate(context);
            }
        }
    }
}