export function SampleInfo(props: {
    children?: any;
    contentTitle?: string;
    sources?: string[];
}): any {
    const samplePagePath = getSamplePagePath();
    const samplePageFile = (samplePagePath?.split('/').pop() ?? "UnknownPage");
    const title = props.contentTitle ?? document.title?.split(' - ').slice(0, -1).join(' - ');
    const markup = (<>
        {title && <section class="content-header">
            <h1>{title}</h1>
        </section>}
        <div class="alert alert-info alert-dismissible s-sample-info">
            <button type="button" class=" btn-close" data-dismiss="alert" data-bs-dismiss="alert" aria-hidden="true"></button>
            {props.children}

            <p class="s-sample-sources">
                <b>Source Files: </b>
                <SampleSourceLink path={samplePageFile + ".tsx"} />
                {props.sources?.map(s => {
                    return <>, <SampleSourceLink path={s.match(/^.[a-z]+$/) ? (samplePageFile + s) : s} /></>
                })}
            </p>
        </div>
    </>);

    document.querySelector("section.content")?.prepend(markup);
    return markup;
}

export function SampleSourceLink(props: { path: string }) {
    let filename = props.path.split('/').pop();
    let href = (document.querySelector('meta[name="repository-blob-url"]')?.getAttribute('content') ?? '#/');;
    if (props.path?.startsWith("/")) {
        href += props.path.substring(1);
    }
    else { 
        const sampleFolder = getSamplePagePath()?.split('/').slice(0, -1).join('/');
        href += "common-features/src/demo.basicsamples/" + sampleFolder + "/" + props.path;
    };
    return <a class="s-sample-source-link" target="_blank" href={href}>{filename}</a>
}

let samplePagePath: string;

export function getSamplePagePath() {

    if (samplePagePath !== void 0)
        return samplePagePath;

    samplePagePath = document.querySelector("meta[name='sample-page-path']")?.getAttribute("content") ?? "";
    samplePagePath = samplePagePath.replace(/^~/, "")
    samplePagePath = samplePagePath.replace(/^\/Serenity.Demo.BasicSamples\/esm\//, '');
    samplePagePath = samplePagePath.replace(/\.js$/, '');
    return samplePagePath;
}