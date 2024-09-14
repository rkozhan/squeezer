﻿namespace Squeezer.Client.Dtos
{
    public class LinkDto
    {
        public long Id { get; set; }
        public string LongUrl { get; set; }
        public string ShortUrl { get; set; }
        public bool IsActive { get; set; }
    }
}
