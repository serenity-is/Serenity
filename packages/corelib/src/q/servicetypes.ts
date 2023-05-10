export interface ServiceError {
    Code?: string;
    Arguments?: string;
    Message?: string;
    Details?: string;
    ErrorId?: string;
}               
      
export interface ServiceResponse {
    Error?: ServiceError;
}
    
export interface ServiceRequest {
}
    
export interface ServiceOptions<TResponse extends ServiceResponse> extends JQueryAjaxSettings {
    request?: any;
    service?: string;
    blockUI?: boolean;
    onError?(response: TResponse): void;
    onSuccess?(response: TResponse): void;
    onCleanup?(): void;
}

export interface SaveRequest<TEntity> extends ServiceRequest {
    EntityId?: any;
    Entity?: TEntity;
    Localizations?: any;
}
    
export interface SaveRequestWithAttachment<TEntity> extends SaveRequest<TEntity> {
    Attachments?: any[];
}
    
export interface SaveResponse extends ServiceResponse {
    EntityId?: any;
}
    
export interface SaveWithLocalizationRequest<TEntity> extends SaveRequest<TEntity> {
    Localizations?: { [key: string]: TEntity };
}
    
export interface DeleteRequest extends ServiceRequest {
    EntityId?: any;
}
    
export interface DeleteResponse extends ServiceResponse {
}
    
export interface UndeleteRequest extends ServiceRequest {
    EntityId?: any;
}
    
export interface UndeleteResponse extends ServiceResponse {
}
    
export enum ColumnSelection {
    List = 0,
    KeyOnly = 1,
    Details = 2,
    None = 3,
    IdOnly = 4,
    Lookup = 5
}
    
export enum RetrieveColumnSelection {
    details = 0,
    keyOnly = 1,
    list = 2,
    none = 3,
    idOnly = 4,
    lookup = 5
}
    
export interface ListRequest extends ServiceRequest {
    Skip?: number;
    Take?: number;
    Sort?: string[];
    ContainsText?: string;
    ContainsField?: string;
    Criteria?: any[];
    EqualityFilter?: any;
    IncludeDeleted?: boolean;
    ExcludeTotalCount?: boolean;
    ColumnSelection?: ColumnSelection;
    IncludeColumns?: string[];
    ExcludeColumns?: string[];
    ExportColumns?: string[];
    DistinctFields?: string[];
}
    
export interface ListResponse<TEntity> extends ServiceResponse {
    Entities?: TEntity[];
    Values?: any[];
    TotalCount?: number;
    Skip?: number;
    Take?: number;
}
    
export interface RetrieveRequest extends ServiceRequest {
    EntityId?: any;
    ColumnSelection?: RetrieveColumnSelection;
    IncludeColumns?: string[];
    ExcludeColumns?: string[];
}
    
export interface RetrieveResponse<TEntity> extends ServiceResponse {
    Entity?: TEntity;
}
    
export interface RetrieveLocalizationRequest extends RetrieveRequest {
}
    
export interface RetrieveLocalizationResponse<TEntity> extends ServiceResponse {
    Entities?: { [key: string]: TEntity };
}
