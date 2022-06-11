import { MetaMetricsRangeInfo } from "./range-info.model";

export interface MetaMetricsMeasurementRequest {
    sublicense: string;
    range: MetaMetricsRangeInfo;
}