import { DataGrid, ListRequest, ToolButton, deepClone, faIcon, postToService } from "@serenity-is/corelib";

export interface ExcelExportOptions {
    grid: DataGrid<any, any>;
    service: string;
    onViewSubmit: () => boolean;
    editRequest?: (request: ListRequest) => ListRequest;
    title?: string;
    hint?: string;
    separator?: boolean;
}

export namespace ExcelExportHelper {

    export function createToolButton(options: ExcelExportOptions): ToolButton {

        return {
            hint: options.hint ?? 'Excel',
            title: options.title ?? '',
            cssClass: 'export-xlsx-button',
            icon: faIcon("file-excel"),
            onClick: function () {
                if (!options.onViewSubmit()) {
                    return;
                }

                let grid = options.grid;

                var request = deepClone(grid.getView().params) as ListRequest;
                request.Take = 0;
                request.Skip = 0;
                var sortBy = grid.getView().sortBy;
                if (sortBy) {
                    request.Sort = sortBy;
                }

                request.ExportColumns = [];
                let columns = grid.getGrid().getColumns();
                for (let column of columns) {
                    request.ExportColumns.push(column.id || column.field);
                }

                if (options.editRequest)
                    request = options.editRequest(request);

                postToService({ service: options.service, request: request, target: '_blank' });
            },
            separator: options.separator
        };
    }
}