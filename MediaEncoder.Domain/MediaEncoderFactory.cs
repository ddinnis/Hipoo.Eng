namespace MediaEncoder.Domain
{
    public class MediaEncoderFactory
    {
        private readonly IEnumerable<IMediaEncoder> mediaEncoders;
        public MediaEncoderFactory(IEnumerable<IMediaEncoder> mediaEncoders) 
        {
           this.mediaEncoders = mediaEncoders;
        }

        public IMediaEncoder? Create(string outputFormat) {
            foreach (var mediaEncoder in mediaEncoders)
            {
                if (mediaEncoder.Accept(outputFormat)) 
                {
                    return mediaEncoder;
                }
            }
            return null;
        }
    }
}
