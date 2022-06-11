using System;

namespace MetaMetrics.Api
{   
    
    public class MetaMetricsMetaKISAttribute : Attribute
    {
        
    }    
    
    public class MetaMetricsMetaTEXTAttribute : Attribute
    {
        
    }
    public class MetaMetricsCounterAttribute : Attribute
    {
        
    }  
    public class MetaMetricsItemAttribute : Attribute
    {
        
    }    
    public class MetaMetricsImportAttribute : Attribute
    {
        
    }
    public class MetaMetricsTitleAttribute : Attribute
    {
        public string Title { set; get; }
        public string Description { set; get; }
    }
}