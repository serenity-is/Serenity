/** Inspired from https://github.com/silverwind/uppie and https://github.com/GoogleChromeLabs/file-drop/blob/master/lib/filedrop.ts */

import { isArrayLike } from "./system";

export interface UploaderOptions {
    accept?: string;
    callback: (e: Event, fd?: FormData, files?: string[]) => void;
    dropZone?: HTMLElement | ArrayLike<HTMLElement>;
    input?: HTMLInputElement;
    multiple?: boolean;
    name?: string;
}

function alwaysTrue() { return true; }

export class Uploader {

    private opt: UploaderOptions;

    constructor(opt: UploaderOptions) {
        this.opt = opt = Object.assign({}, Uploader.defaults, opt);

        if (opt.input) {
            this.watchInput(opt.input);
        }

        if (isArrayLike(opt.dropZone)) {
            for (var i = 0; i < opt.dropZone.length; i++)
                opt.dropZone[i] && this.watchDropZone(opt.dropZone[i]);
        }
        else if (opt.dropZone) {
            this.watchDropZone(opt.dropZone);
        }
    }

    static defaults: Partial<UploaderOptions> = {
        name: "files[]"
    }

    private isMultiple() {
        return !!(this.opt.multiple ?? this.opt?.input?.getAttribute("multiple"));
    }

    private getTypePredicate(): ((type: string) => boolean) {
        let acceptVal = this.opt.accept ?? this.opt?.input?.getAttribute("accept")        
        if (!acceptVal)
            return alwaysTrue;
    
        const accepts = acceptVal.toLowerCase().split(',').map((accept) => {
            return accept.split('/').map(part => part.trim());
        }).filter(acceptParts => acceptParts.length === 2);
    
        return (type: string) => {
            const [typeMain, typeSub] = (type ?? "").toLowerCase().split('/').map(s => s.trim());
    
            for (const [acceptMain, acceptSub] of accepts) {
                if (typeMain === acceptMain && (acceptSub === '*' || typeSub === acceptSub)) {
                    return true;
                }
            }
            return false;
        };
    }
    
    private getMatchingItems(list: DataTransferItemList): DataTransferItem[] {
        let predicate = this.getTypePredicate();
        let results: DataTransferItem[] = Array.from(list ?? []).filter(x => x.kind === "file" && predicate(x.type));
        return this.isMultiple() ? results : [results[0]];
    }

    private watchInput(input: HTMLInputElement) {
        input.addEventListener("change", e => {
            if ((e.target as any)?.files?.length) {
                this.arrayApi(e, (e.target as HTMLInputElement).files);
            } else {
                this.opt.callback(e);
            }
        });
    }

    private watchDropZone(node: HTMLElement) {
        const stop = (e: Event) => e.preventDefault();
        node.addEventListener("dragover", stop);
        node.addEventListener("dragenter", e => {
            (node as any).dragEnterCount = ((node as any).dragEnterCount || 0) + 1;
            if ((node as any).dragEnterCount > 1) {
                return;
            }
            if (e.dataTransfer === null) {
                node.classList.add('drop-invalid');
                return;
            }

            const matchingFiles = this.getMatchingItems(e.dataTransfer.items);
            // Safari doesn't give file information on drag enter, so the best we can do is return valid.
            const validDrop: boolean = e.dataTransfer && e.dataTransfer.items.length ? (matchingFiles[0] !== undefined) : true;

            node.classList.toggle('drop-valid', !!validDrop);
            node.classList.toggle('drop-invalid', !validDrop);
        });

        node.addEventListener("dragleave", e => {
            (node as any).dragEnterCount = Math.max(((node as any).dragEnterCount || 0) - 1, 0);
            if ((node as any).dragEnterCount === 0) {
                (node as any).dragEnterCount = 0;
                node.classList.remove('drop-valid');
                node.classList.remove('drop-invalid');
                return;
            }
            if (e.dataTransfer === null) {
                node.classList.add('drop-invalid');
                return;
            }
        });

        node.addEventListener("drop", (e) => {
            e.preventDefault();
            if (e.dataTransfer.items?.[0]?.webkitGetAsEntry()) {
                this.entriesApi(e, e.dataTransfer.items);
            } else if (e.dataTransfer.files) {
                this.arrayApi(e, e.dataTransfer.files);
            } else {
                this.opt.callback(e);
            }
        });
    }

    private arrayApi(e: Event, fileList: FileList) {
        const fd = new FormData();
        const files = [];

        let predicate = this.getTypePredicate();
        let filteredFiles = Array.from(fileList).filter(x => predicate(x.type));
        if (!this.isMultiple() && filteredFiles.length > 0)
            filteredFiles = [filteredFiles[0]];
        for (var file of filteredFiles) {
            fd.append(this.opt.name, file, file.webkitRelativePath || file.name);
            files.push(file.webkitRelativePath || file.name);
        }
        this.opt.callback?.(e, fd, files);
    }

    private entriesApi(e: Event, items: DataTransferItemList) {
        const fd = new FormData();
        const files: string[] = [];
        const rootPromises: Promise<any>[] = [];
        let predicate = this.getTypePredicate();
        let multiple = this.isMultiple();

        let readDirectory = (entry: FileSystemDirectoryEntry, path: string, resolve: Function) => {
            if (!path)
                path = entry.name;
            readEntries(entry, null, null, entries => {
                const promises = [];
                for (const entry of entries) {
                    if (!multiple && files.length)
                        break;
                    promises.push(new Promise(resolve => {
                        if (entry.isFile) {
                            (entry as FileSystemFileEntry).file(file => {
                                const p = `${path}/${file.name}`;
                                if ((multiple || !files.length) && predicate(file.type)) {
                                    fd.append(this.opt.name, file, p);
                                    files.push(p);
                                }
                                resolve(void 0);
                            }, resolve.bind(void 0));
                        } else {
                            readDirectory(entry, `${path}/${entry.name}`, resolve);
                        }
                    }));
                }
                Promise.all(promises).then(resolve.bind(void 0));
            });
        }

        let readEntries = (entry: FileSystemDirectoryEntry, reader: FileSystemDirectoryReader, oldEntries: FileSystemEntry[], cb: (entries: any) => void) => {
            const dirReader = reader || entry.createReader();

            dirReader.readEntries(entries => {
                if (!multiple && files.length)
                    return;
                const newEntries = oldEntries ? oldEntries.concat(entries) : entries;
                if (entries.length) {
                    setTimeout(readEntries.bind(null, entry, dirReader, newEntries, cb), 0);
                } else {
                    cb(newEntries);
                }
            });
        }

        for (var i = 0; i < items.length; i++) {
            if (!multiple && files.length)
                return;

            let entry = items[i].webkitGetAsEntry();
            if (entry) {
                rootPromises.push(new Promise((resolve) => {
                    if (entry.isFile) {
                        (entry as FileSystemFileEntry).file((file) => {
                            if ((multiple || !files.length) && predicate(file.type)) {
                                fd.append(this.opt.name, file, file.name);
                                files.push(file.name);
                            }
                            resolve(void 0);
                        }, resolve.bind(void 0));
                    } else if (entry.isDirectory) {
                        readDirectory(entry as FileSystemDirectoryEntry, null, resolve);
                    }
                }));
            }
        }

        Promise.all(rootPromises).then(() => this.opt.callback(e, fd, files));
    }
}

