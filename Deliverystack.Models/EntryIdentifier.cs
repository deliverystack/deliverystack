namespace Deliverystack.Models
{
    using Deliverystack.Interfaces;

    public class EntryIdentifier : IEntryIdentifier
    {
        public EntryIdentifier(string contentType, string entryUid)
        {
            _contentType = contentType;
            _entryUid = entryUid;
        }

        readonly private string _contentType;
        readonly private string _entryUid; 

        public string GetContentType()
        {
            return _contentType;
        }

        public string GetEntryId()
        {
            return _entryUid;
        }
    }
}
