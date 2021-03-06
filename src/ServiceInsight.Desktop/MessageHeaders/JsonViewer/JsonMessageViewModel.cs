﻿using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Desktop.Events;
using NServiceBus.Profiler.Desktop.Models;

namespace NServiceBus.Profiler.Desktop.MessageViewers.JsonViewer
{
    public class JsonMessageViewModel : Screen, IJsonMessageViewModel
    {
        private IJsonMessageView _messageView;

        protected override void OnActivate()
        {
            base.OnActivate();
            DisplayName = "Json";
        }

        public override void AttachView(object view, object context)
        {
            base.AttachView(view, context);
            _messageView = (IJsonMessageView)view;
            OnSelectedMessageChanged();
        }

        public MessageBody SelectedMessage { get; set; }

        public void OnSelectedMessageChanged()
        {
            if (_messageView == null) return;

            _messageView.Clear();

            if (SelectedMessage == null || SelectedMessage.Body == null) return;

            _messageView.Display(SelectedMessage.Body);
        }

        public void Handle(SelectedMessageChanged @event)
        {
            SelectedMessage = @event.Message;
        }
    }
}