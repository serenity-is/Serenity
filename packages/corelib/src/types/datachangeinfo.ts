export {}

export interface DataChangeInfo extends Event {
    type: string;
    entityId: any;
    entity: any;
}
