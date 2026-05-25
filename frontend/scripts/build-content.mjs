#!/usr/bin/env node
/**
 * Walks each backend project, reads every .cs file, merges with hand-curated
 * annotations from `frontend/content-src/<slug>/annotations.json`, and emits
 * `frontend/public/content/<slug>/files.json` consumed by the frontend's
 * StructureSection.
 *
 * Run via `npm run build:content` (hooked into prestart/prebuild).
 */
import { readdirSync, readFileSync, writeFileSync, existsSync, mkdirSync } from 'node:fs';
import { join, relative, dirname } from 'node:path';
import { fileURLToPath } from 'node:url';

const __dirname = dirname(fileURLToPath(import.meta.url));
const FRONTEND_ROOT = join(__dirname, '..');
const REPO_ROOT = join(__dirname, '..', '..');

const SLUG_TO_FOLDER = {
  mvc: 'ArchStudy.Mvc',
  'feature-based': 'ArchStudy.FeatureBased',
  'vertical-slice': 'ArchStudy.VerticalSlice',
  onion: 'ArchStudy.Onion',
  hexagonal: 'ArchStudy.Hexagonal',
  clean: 'ArchStudy.Clean',
  ddd: 'ArchStudy.Ddd',
  cqrs: 'ArchStudy.Cqrs',
};

const SKIP_DIRS = new Set(['bin', 'obj']);

function walkCsFiles(dir, projectRoot) {
  const out = [];
  for (const entry of readdirSync(dir, { withFileTypes: true })) {
    const full = join(dir, entry.name);
    if (entry.isDirectory()) {
      if (SKIP_DIRS.has(entry.name)) continue;
      out.push(...walkCsFiles(full, projectRoot));
    } else if (entry.isFile() && entry.name.endsWith('.cs')) {
      out.push(relative(projectRoot, full).split('\\').join('/'));
    }
  }
  return out;
}

function loadAnnotations(slug) {
  const path = join(FRONTEND_ROOT, 'content-src', slug, 'annotations.json');
  if (!existsSync(path)) return {};
  try {
    return JSON.parse(readFileSync(path, 'utf8'));
  } catch (err) {
    console.warn(`[build-content] could not parse ${path}: ${err.message}`);
    return {};
  }
}

function ensureDir(p) {
  if (!existsSync(p)) mkdirSync(p, { recursive: true });
}

let totalFiles = 0;
let totalProjects = 0;

for (const [slug, folderName] of Object.entries(SLUG_TO_FOLDER)) {
  const projectRoot = join(REPO_ROOT, 'backend', folderName);
  if (!existsSync(projectRoot)) {
    console.warn(`[build-content] skip ${slug}: ${projectRoot} not found`);
    continue;
  }

  const annotations = loadAnnotations(slug);
  const paths = walkCsFiles(projectRoot, projectRoot).sort();
  const files = paths.map((p) => ({
    path: p,
    annotation: annotations[p] ?? null,
    content: readFileSync(join(projectRoot, p), 'utf8'),
  }));

  const payload = { root: folderName, files };
  const outDir = join(FRONTEND_ROOT, 'public', 'content', slug);
  ensureDir(outDir);
  writeFileSync(join(outDir, 'files.json'), JSON.stringify(payload, null, 2), 'utf8');

  console.log(`[build-content] ${slug.padEnd(16)} ${files.length} files`);
  totalFiles += files.length;
  totalProjects += 1;
}

console.log(`[build-content] done · ${totalFiles} files across ${totalProjects} architectures`);
