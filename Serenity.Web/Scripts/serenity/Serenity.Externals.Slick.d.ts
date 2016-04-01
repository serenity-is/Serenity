declare namespace Slick.Data {
    function GroupItemMetadataProvider(): void;
}
declare namespace Slick {
    function Event(): void;
    function EventData(): void;
    function Group(): void;
    function GroupTotals(): void;
}
declare namespace Slick.Data {
    interface RemoteViewOptions {
    }
    class RemoteView {
        constructor(options: any);
    }
}
declare namespace Slick.Data.Aggregators {
    function Avg(field: any): void;
    function WeightedAvg(field: any, weightedField: any): void;
    function Min(field: any): void;
    function Max(field: any): void;
    function Sum(field: any): void;
}
