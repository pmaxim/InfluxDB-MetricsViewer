using System;

namespace MetaMetrics.Api
{
    public interface IMetaMetricsTimeValue
    {
        string FromTitle { get; } 
        string Title { get; } 
        DateTime From {  get; } 
        DateTime Till { get; }
        long Value { set; get; }
        bool Exists { set;get; }
        
    }
}