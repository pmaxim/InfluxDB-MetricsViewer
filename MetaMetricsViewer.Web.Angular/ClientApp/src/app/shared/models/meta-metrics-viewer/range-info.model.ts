export interface MetaMetricsRangeInfo {
    display: string;
    days?: number;
    weeks?: number;
    monthes?: number;
    years?: number;
    default?: boolean;
    currentWeek?: boolean;
    lastWeek?: boolean;
    currentMonth?: boolean;
    lastMonth?: boolean;
    currentQuarter?: boolean;
    lastQuarter?: boolean;
    currentYear?: boolean;
    lastYear?: boolean;
    every?: string;
}

export function ranges(): MetaMetricsRangeInfo[] {
  return [
    { display: 'Today', days: 0, default: true, every:'15m' },
    { display: 'Yesterday', days: 1,  every: '30m' },
    { display: '2d', days: 2, every: '1h' },
    { display: '3d', days: 3, every: '1h' },
    { display: '4d', days: 4, every: '2h' },
    { display: '1w', weeks: 1, every: '3h' },
    { display: '2w', weeks: 2, every: '6h' },
    { display: '4w', weeks: 4, every: '12h' },
    { display: '1m', monthes: 1, every: '1d' },
    { display: '2m', monthes: 2, every: '2d' },
    { display: '3m', monthes: 3, every: '3d' },
    //{ display: '4m', monthes: 4, every: '4d' },
    //{ display: '6m', monthes: 6, every: '6d' },
    //{ display: '1y', years: 1, every: '1mo' },
    { display: 'Current week', currentWeek: true, every: '5h' },
    { display: 'Current month', currentMonth: true, every: '1d' },
    { display: 'Current quarter', currentQuarter: true, every: '3d' },
    //{ display: 'Current year', currentYear: true, every: '1mo' },
    { display: 'Last week', lastWeek: true, every: '5h' },
    { display: 'Last month', lastMonth: true, every: '1d' },
    { display: 'Last quarter', lastQuarter: true, every: '3d' },
    //{ display: 'Last year', lastYear: true, every: '1mo' }
  ];
}
