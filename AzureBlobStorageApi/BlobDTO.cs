namespace AzureBlobStorageApi
{
    public class BlobDTO
    {
        public BlobDTO(string name, string type, string uri)
        {
            Name = name;
            Type = type;
            Uri = uri;
        }

        public string Name { get; set; }
        public string Type { get; set; }
        public string Uri { get; set; }
    }
}
