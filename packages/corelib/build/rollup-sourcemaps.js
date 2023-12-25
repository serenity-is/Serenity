// adapted from https://github.com/IIIMADDINIII/rollup-plugin-include-sourcemaps

import { createFilter } from '@rollup/pluginutils';
import fs from 'fs';
import * as urlLib from 'url';
import { promisify } from 'util';

function resolveUrl(...args) {
  return args.reduce((resolved, nextUrl) => urlLib.resolve(resolved, nextUrl));
}

function parseMapToJSON(str) {
  return JSON.parse(str.replace(/^\)\]\}'/, ''));
}

const sourceMappingURLRegex = RegExp(
  '(?:/\\*(?:\\s*\\r?\\n(?://)?)?(?:[#@] sourceMappingURL=([^\\s\'"]*))\\s*\\*/|//(?:[#@] sourceMappingURL=([^\\s\'"]*)))\\s*',
);

function getSourceMappingUrl(code) {
  const match = sourceMappingURLRegex.exec(code);
  return match ? match[1] || match[2] || '' : null;
}

async function resolveSourceMap(code, codeUrl, read) {
  const sourceMappingURL = getSourceMappingUrl(code);
  if (!sourceMappingURL) {
    return null;
  }
  const dataUri = /^data:([^,;]*)(;[^,;]*)*(?:,(.*))?$/.exec(sourceMappingURL);
  if (dataUri) {
    const mimeType = dataUri[1] || 'text/plain';
    if (!/^(?:application|text)\/json$/.test(mimeType)) {
      throw new Error('Unuseful data uri mime type: ' + mimeType);
    }
    const map = parseMapToJSON(decodeURIComponent(dataUri[3] || ''));
    return { sourceMappingURL, url: null, sourcesRelativeTo: codeUrl, map };
  }
  const url = resolveUrl(codeUrl, sourceMappingURL);
  const map = parseMapToJSON(String(await read(decodeURIComponent(url))));
  return { sourceMappingURL, url, sourcesRelativeTo: url, map };
}

async function resolveSources(map, mapUrl, read) {
  const sourcesResolved = [];
  const sourcesContent = [];
  for (let index = 0, len = map.sources.length; index < len; index++) {
    const sourceRoot = map.sourceRoot;
    const sourceContent = (map.sourcesContent || [])[index];
    const resolvePaths = [mapUrl, map.sources[index]];
    if (sourceRoot !== undefined && sourceRoot !== '') {
      resolvePaths.splice(1, 0, sourceRoot.replace(/\/?$/, '/'));
    }
    sourcesResolved[index] = resolveUrl(...resolvePaths);
    if (typeof sourceContent === 'string') {
      sourcesContent[index] = sourceContent;
      continue;
    }
    try {
      const source = await read(customDecodeUriComponent(sourcesResolved[index]));
      sourcesContent[index] = String(source);
    } catch (error) {
      sourcesContent[index] = error;
    }
  }
  return { sourcesResolved, sourcesContent };
}

export default function sourcemaps({ include, exclude, readFile = fs.readFile } = {}) {
  const filter = createFilter(include, exclude);
  const promisifiedReadFile = promisify(readFile);

  return {
    name: 'sourcemaps',

    async load(id) {
      if (!filter(id)) {
        return null;
      }

      let code;
      try {
        code = (await promisifiedReadFile(id)).toString();
      } catch {
        this.warn('Failed reading file');
        return null;
      }

      let map;
      try {
        const result = await resolveSourceMap(code, id, promisifiedReadFile);

        // The code contained no sourceMappingURL
        if (result === null) {
          return code;
        }

        map = result.map;
      } catch {
        this.warn('Failed resolving source map');
        return code;
      }

      // Resolve sources if they're not included
      if (map.sourcesContent === undefined) {
        try {
          const { sourcesContent } = await resolveSources(map, id, promisifiedReadFile);
          if (sourcesContent.every(item => typeof item === 'string')) {
            map.sourcesContent = sourcesContent;
          }
        } catch {
          this.warn('Failed resolving sources for source map');
        }
      }

      return { code, map };
    },
  };
}
