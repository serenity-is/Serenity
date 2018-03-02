namespace Serenity.UI {    
    export interface FormDataModel<TEntity> {
        entity: TEntity;
        entityId?: any;
        formMode: FormMode;
        formTitle?: string;
        onSave?: (values: TEntity) => PromiseLike<void>;
        onDelete?: () => PromiseLike<void>;
        onUndelete?: () => PromiseLike<void>;
        onReload?: () => PromiseLike<void>;
    }
}