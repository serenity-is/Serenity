export {}

export interface DataChangeInfo extends Event {
    operationType: string;
    entityId: any;
    entity: any;
}
