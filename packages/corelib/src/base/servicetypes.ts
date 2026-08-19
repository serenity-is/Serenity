/**
 * Error payload returned by Serenity service endpoints inside a {@link ServiceResponse}.
 * The server populates at least `Code` or `Message`; other fields are optional and
 * depend on the handler / validation layer.
 */
export interface ServiceError {
    /** Machine-readable error code (e.g. `"NotLoggedIn"`, `"ValidationError"`, `"AccessDenied"`). */
    Code?: string;
    /** Optional comma-separated or serialized arguments that parameterize the error message (e.g. field names). */
    Arguments?: string;
    /** Human-readable, possibly localized, error message suitable for display. */
    Message?: string;
    /** Detailed / technical information (e.g. stack trace or inner exception) — only when diagnostics are enabled. */
    Details?: string;
    /** Correlation / error ID assigned server-side for log lookup. */
    ErrorId?: string;
}

/**
 * Base contract for every Serenity service response.
 * Successful responses omit `Error`; failed responses populate it and may omit other fields.
 */
export interface ServiceResponse {
    /** Error information when the request failed; `undefined` on success. */
    Error?: ServiceError;
}

/**
 * Marker base for all Serenity service request DTOs.
 * Concrete requests extend this to add handler-specific fields.
 */
export interface ServiceRequest {
}

/**
 * Request DTO for `Create` / `Update` service handlers.
 * @typeParam TEntity - Row / entity type being saved.
 */
export interface SaveRequest<TEntity> extends ServiceRequest {
    /** Primary key of the entity to update; omit for inserts (server generates the key). */
    EntityId?: any;
    /** Entity fields to persist. For updates only modified fields need to be sent, depending on handler. */
    Entity?: TEntity;
    /** Per-language patches for localizable rows, keyed by language ID (e.g. `{ "en": { Name: "Hello" } }`). */
    Localizations?: { [languageId: string]: Partial<TEntity> };
}

/**
 * Response DTO for `Create` / `Update` handlers.
 */
export interface SaveResponse extends ServiceResponse {
    /** Primary key of the created / updated entity as assigned / confirmed by the server. */
    EntityId?: any;
}

/**
 * Request DTO for `Delete` handlers (soft or hard delete depending on row / handler).
 */
export interface DeleteRequest extends ServiceRequest {
    /** Primary key of the entity to delete. */
    EntityId?: any;
}

/**
 * Response DTO for `Delete` handlers. No additional fields beyond {@link ServiceResponse}.
 */
export interface DeleteResponse extends ServiceResponse {
}

/**
 * Request DTO for `Undelete` handlers (restores a soft-deleted row).
 */
export interface UndeleteRequest extends ServiceRequest {
    /** Primary key of the entity to undelete. */
    EntityId?: any;
}

/**
 * Response DTO for `Undelete` handlers. No additional fields beyond {@link ServiceResponse}.
 */
export interface UndeleteResponse extends ServiceResponse {
}

/**
 * Controls which columns are selected when listing entities.
 * Mirrors the server-side `ColumnSelection` enum and is used by `ListRequest.ColumnSelection`.
 */
export enum ColumnSelection {
    /** Default view columns (what the grid shows). */
    List = 0,
    /** Only key (ID) columns. */
    KeyOnly = 1,
    /** Detail view columns (more fields than `List`). */
    Details = 2,
    /** No columns (only aggregates / counts). */
    None = 3,
    /** Only the identity column. */
    IdOnly = 4,
    /** Columns needed for lookup display (usually ID + display field). */
    Lookup = 5
}

/**
 * Column-selection variant used by `Retrieve` handlers.
 * Lowercase naming is kept for backward compatibility with legacy endpoints.
 */
export enum RetrieveColumnSelection {
    /** Detail columns. */
    details = 0,
    /** Only key columns. */
    keyOnly = 1,
    /** List columns. */
    list = 2,
    /** No columns. */
    none = 3,
    /** Only the identity column. */
    idOnly = 4,
    /** Lookup columns. */
    lookup = 5
}

/**
 * Request DTO for `List` handlers (grid data source).
 * Supports paging, sorting, filtering and column selection.
 */
export interface ListRequest extends ServiceRequest {
    /** Number of records to skip (offset) for paging. */
    Skip?: number;
    /** Maximum number of records to take (page size). Omit or 0 for server default. */
    Take?: number;
    /** Sort expressions, e.g. `["Name ASC", "Age DESC"]`. */
    Sort?: string[];
    /** Quick-search text applied across searchable fields (or {@link ContainsField} when specified). */
    ContainsText?: string;
    /** When set, `ContainsText` is applied only to this field instead of all searchable fields. */
    ContainsField?: string;
    /** Advanced filter criteria tree (Serenity `Criteria` format: `[field, op, value]` or nested `["and", [...]]`). */
    Criteria?: any[];
    /** Simple equality filter map, e.g. `{ Status: 1 }`. Combined with `Criteria` via AND. */
    EqualityFilter?: any;
    /** When true, soft-deleted rows are included in results. */
    IncludeDeleted?: boolean;
    /** When true the server may skip computing `TotalCount` for performance. */
    ExcludeTotalCount?: boolean;
    /** Preset that controls which columns are returned. See {@link ColumnSelection}. */
    ColumnSelection?: ColumnSelection;
    /** Explicit allow-list of columns to include (overrides `ColumnSelection`). */
    IncludeColumns?: string[];
    /** Explicit deny-list of columns to exclude. */
    ExcludeColumns?: string[];
    /** Columns to export when `List` is used for export; defaults to visible columns. */
    ExportColumns?: string[];
    /** Fields to apply `DISTINCT` on (server-dependent). */
    DistinctFields?: string[];
    /** Language ID for localized field selection (e.g. `"en"`, `"tr"`). */
    Localize?: string;
}

/**
 * Response DTO for `List` handlers.
 * @typeParam TEntity - Row / entity type of the listed records.
 */
export interface ListResponse<TEntity> extends ServiceResponse {
    /** Page of entities matching the request. */
    Entities?: TEntity[];
    /** Alternative `Values` array used by some handlers that return raw values instead of entities. */
    Values?: any[];
    /** Total number of records matching the filter (before paging), unless `ExcludeTotalCount` was set. */
    TotalCount?: number;
    /** Echo of `Skip` from the request. */
    Skip?: number;
    /** Echo of `Take` from the request. */
    Take?: number;
}

/**
 * Request DTO for `Retrieve` handlers (single-entity fetch).
 */
export interface RetrieveRequest extends ServiceRequest {
    /** Primary key of the entity to retrieve. */
    EntityId?: any;
    /** Preset that controls which columns are returned. See {@link RetrieveColumnSelection}. */
    ColumnSelection?: RetrieveColumnSelection;
    /** Explicit allow-list of columns to include. */
    IncludeColumns?: string[];
    /** Explicit deny-list of columns to exclude. */
    ExcludeColumns?: string[];
}

/**
 * Response DTO for `Retrieve` handlers.
 * @typeParam TEntity - Row / entity type.
 */
export interface RetrieveResponse<TEntity> extends ServiceResponse {
    /** The retrieved entity, or `undefined` if not found (depending on handler). */
    Entity?: TEntity;
    /** Per-language values for localizable fields, keyed by language ID. */
    Localizations?: { [languageId: string]: Partial<TEntity> };
}

/**
 * HTTP-level error details supplied to {@link ServiceOptions.onError} alongside the service error payload.
 */
export interface RequestErrorInfo {
    /** HTTP status code (e.g. `403`, `500`). */
    status?: number;
    /** HTTP status text (e.g. `"Forbidden"`). */
    statusText?: string;
    /** Raw response body text when the response could not be parsed as JSON. */
    responseText?: string;
}

/**
 * Options for {@link serviceCall} / {@link serviceRequest} / `serviceFetch`.
 * Extends the native `RequestInit` so any `fetch` option (e.g. `signal`, `cache`) can be passed through.
 * @typeParam TResponse - Expected service response type (must extend {@link ServiceResponse}).
 */
export interface ServiceOptions<TResponse extends ServiceResponse> extends RequestInit {
    /** When true (default), a `403` with a `Location` header triggers a top-window redirect. @defaultValue `true` */
    allowRedirect?: boolean;
    /** When `false` a synchronous XHR is used (blocks the UI). Prefer `true` (default). @defaultValue `true` */
    async?: boolean;
    /** When true (default) the UI is blocked with a loading indicator for the duration of the request. @defaultValue `true` */
    blockUI?: boolean;
    /** Extra HTTP headers merged with defaults (`Accept: application/json`, `Content-Type: application/json`, `X-CSRF-TOKEN` when same-origin). */
    headers?: Record<string, string>;
    /** Request DTO serialized as JSON in the POST body. */
    request?: any;
    /** Service endpoint key (e.g. `"Administration/User/List"`). Resolved via `~/Services/` when relative. Mutually exclusive with `url`. */
    service?: string;
    /** Absolute or `~/`-prefixed URL. When provided `service` is ignored. */
    url?: string;
    /** How service errors are surfaced to the user. `"alert"` shows a dialog, `"notification"` shows a toast, `"none"` suppresses default handling. */
    errorMode?: 'alert' | 'notification' | 'none';
    /** Callback invoked after the request finishes regardless of success or failure (after `blockUI` is undone). */
    onCleanup?(): void;
    /**
     * Custom error handler. Return `true` to indicate the error was handled and suppress default error display.
     * @param response - Parsed service response (may be `null` for network / HTTP errors).
     * @param info - HTTP-level error details.
     * @returns `true` if handled (prevents double notification), `false`/`void` to allow default handling.
     */
    onError?(response: TResponse, info?: RequestErrorInfo): void | boolean;
    /**
     * Success callback invoked with the parsed response when `Error` is not set.
     * @param response - Successful service response.
     */
    onSuccess?(response: TResponse): void;
}
