const fs = require('fs');
const path = require('path');

const root = path.resolve(__dirname, '..');
const htmlPath = path.join(root, 'docs', 'GUIA_COMPLETA_FACCISTATS.html');
const outDir = path.join(root, 'output', 'pdf');
const output = path.join(outDir, 'Guia_Completa_FacciStats.pdf');
fs.mkdirSync(outDir, { recursive: true });

let html = fs.readFileSync(htmlPath, 'utf8');
html = html.replace(/<style[\s\S]*?<\/style>/gi, '').replace(/<script[\s\S]*?<\/script>/gi, '');

const entities = {
  '&amp;': '&', '&lt;': '<', '&gt;': '>', '&quot;': '"', '&#39;': "'",
  '&nbsp;': ' ', '&#10;': '\n', '&rarr;': '->', '&lt;---': '<---'
};
function clean(value) {
  value = value.replace(/<br\s*\/?>/gi, '\n');
  value = value.replace(/<[^>]+>/g, '');
  value = value.replace(/&(?:amp|lt|gt|quot|#39|nbsp|#10|rarr);/gi, x => entities[x.toLowerCase()] ?? x);
  value = value.replace(/&#(\d+);/g, (_, n) => String.fromCharCode(Number(n)));
  value = value.replace(/\r/g, '').replace(/[ \t]+\n/g, '\n');
  return value.trim();
}
function ascii(value) {
  const map = {'á':'a','é':'e','í':'i','ó':'o','ú':'u','ü':'u','ñ':'n','Á':'A','É':'E','Í':'I','Ó':'O','Ú':'U','Ü':'U','Ñ':'N','¿':'?','¡':'!','–':'-','—':'-','“':'"','”':'"','’':"'",'•':'-','→':'->'};
  return Array.from(value).map(c => map[c] || c).join('').replace(/[^\x09\x0A\x0D\x20-\x7E]/g, '?');
}

const blocks = [];
const tokenRe = /<(h1|h2|h3|p|pre|li|tr|div)(?:\s+[^>]*)?>([\s\S]*?)<\/\1>/gi;
let match;
while ((match = tokenRe.exec(html))) {
  const tag = match[1].toLowerCase();
  let text = match[2];
  if (tag === 'tr') {
    const cells = [];
    const cellRe = /<(?:th|td)(?:\s+[^>]*)?>([\s\S]*?)<\/(?:th|td)>/gi;
    let c;
    while ((c = cellRe.exec(text))) cells.push(clean(c[1]).replace(/\n+/g, ' '));
    text = cells.join(' | ');
  } else text = clean(text);
  if (!text) continue;
  if (/^Ubicacion de fuente:/i.test(text)) continue;
  if (tag === 'div' && !/flow|callout|ok/i.test(match[0])) continue;
  let type = tag;
  if (tag === 'div') type = 'note';
  if (tag === 'li') text = '- ' + text;
  if (tag === 'pre') type = 'code';
  if (tag === 'tr') type = 'table';
  blocks.push({ type, text: ascii(text) });
}

const pageW = 595, pageH = 842, left = 48, right = 48, top = 54, bottom = 56;
const usableW = pageW - left - right;
const pages = [];
let page = [];
let y = pageH - top;
function newPage() { if (page.length) pages.push(page); page = []; y = pageH - top; }
function textWidth(text, size) { return text.length * size * 0.50; }
function wrap(text, size, width = usableW) {
  const max = Math.max(18, Math.floor(width / (size * 0.50)));
  const result = [];
  for (const raw of text.split('\n')) {
    if (!raw) { result.push(''); continue; }
    let line = '';
    for (const word of raw.split(/\s+/)) {
      const next = line ? line + ' ' + word : word;
      if (next.length > max && line) { result.push(line); line = word; }
      else line = next;
    }
    if (line) result.push(line);
  }
  return result;
}
function addLine(line, size, font, color, indent = 0) {
  const lineH = size * 1.28;
  if (y - lineH < bottom) newPage();
  page.push({ text: line, size, font, color, x: left + indent, y });
  y -= lineH;
}
function addBlock(block) {
  let size = 9.5, font = 'F1', color = '0.10 0.13 0.18', indent = 0, gap = 5;
  if (block.type === 'h2' && /^11\./.test(block.text) && page.length) newPage();
  if (block.type === 'h1') { size = 25; font = 'F2'; color = '0.04 0.31 0.42'; gap = 12; }
  else if (block.type === 'h2') { size = 16; font = 'F2'; color = '0.04 0.31 0.42'; gap = 12; }
  else if (block.type === 'h3') { size = 11.5; font = 'F2'; color = '0.04 0.31 0.42'; gap = 8; }
  else if (block.type === 'code') { size = 8.2; font = 'F3'; color = '0.91 0.96 0.98'; gap = 6; }
  else if (block.type === 'table') { size = 7.5; color = '0.10 0.13 0.18'; gap = 2; }
  else if (block.type === 'note') { size = 9; color = '0.05 0.31 0.42'; gap = 7; }
  else if (block.type === 'li') { indent = 12; gap = 2; }
  if (block.type === 'code') {
    const lines = [];
    for (const raw of block.text.split('\n')) lines.push(...wrap(raw, size, usableW - 8));
    const h = lines.length * size * 1.25 + 18;
    if (y - h < bottom) newPage();
    page.push({ box: true, x: left - 3, y: y + 4, w: usableW + 6, h, color: '0.08 0.14 0.17' });
    for (const line of lines) addLine(line, size, font, color, 4);
    y -= 18;
    return;
  }
  y -= gap;
  for (const line of wrap(block.text, size, usableW - indent)) addLine(line, size, font, color, indent);
  if (block.type === 'h2') {
    if (y - 4 < bottom) newPage();
    page.push({ rule: true, x: left, y: y + 2, w: usableW, color: '0.13 0.64 0.72' });
    y -= 5;
  }
  y -= 2;
}

for (const block of blocks) addBlock(block);
newPage();

function esc(s) { return s.replace(/\\/g, '\\\\').replace(/\(/g, '\\(').replace(/\)/g, '\\)'); }
const objects = [];
function obj(body) { objects.push(body); return objects.length; }
const fontRegular = obj('<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica /Encoding /WinAnsiEncoding >>');
const fontBold = obj('<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold /Encoding /WinAnsiEncoding >>');
const fontMono = obj('<< /Type /Font /Subtype /Type1 /BaseFont /Courier /Encoding /WinAnsiEncoding >>');
const pageRefs = [];
for (let i = 0; i < pages.length; i++) {
  const commands = [];
  commands.push('1 1 1 rg 0 0 ' + pageW + ' ' + pageH + ' re f');
  commands.push('0.04 0.31 0.42 rg 0 0 ' + pageW + ' 6 re f');
  for (const item of pages[i]) {
    if (item.box) {
      commands.push(item.color + ' rg ' + item.x + ' ' + (item.y - item.h + 3) + ' ' + item.w + ' ' + item.h + ' re f');
    } else if (item.rule) {
      commands.push(item.color + ' RG 1 w ' + item.x + ' ' + item.y + ' m ' + (item.x + item.w) + ' ' + item.y + ' l S');
    } else {
      commands.push(item.color + ' rg BT /' + item.font + ' ' + item.size + ' Tf 1 0 0 1 ' + item.x + ' ' + item.y + ' Tm (' + esc(item.text) + ') Tj ET');
    }
  }
  commands.push('0.35 0.40 0.43 rg BT /F1 8 Tf 1 0 0 1 ' + left + ' 28 Tm (FacciStats - Guia completa de ejecucion y defensa) Tj ET');
  commands.push('0.35 0.40 0.43 rg BT /F1 8 Tf 1 0 0 1 ' + (pageW - 78) + ' 28 Tm (' + (i + 1) + ' / ' + pages.length + ') Tj ET');
  const stream = commands.join('\n');
  const streamRef = obj('<< /Length ' + Buffer.byteLength(stream, 'ascii') + ' >>\nstream\n' + stream + '\nendstream');
  const pageRef = obj('<< /Type /Page /Parent PAGESREF /MediaBox [0 0 ' + pageW + ' ' + pageH + '] /Resources << /Font << /F1 ' + fontRegular + ' 0 R /F2 ' + fontBold + ' 0 R /F3 ' + fontMono + ' 0 R >> >> /Contents ' + streamRef + ' 0 R >>');
  pageRefs.push(pageRef);
}
const kids = pageRefs.map(r => r + ' 0 R').join(' ');
const pagesRef = obj('<< /Type /Pages /Kids [' + kids + '] /Count ' + pageRefs.length + ' >>');
for (const ref of pageRefs) objects[ref - 1] = objects[ref - 1].replace('PAGESREF', pagesRef + ' 0 R');
const catalogRef = obj('<< /Type /Catalog /Pages ' + pagesRef + ' 0 R >>');
let pdf = '%PDF-1.4\n%\xE2\xE3\xCF\xD3\n';
const offsets = [0];
for (let i = 0; i < objects.length; i++) { offsets[i + 1] = Buffer.byteLength(pdf, 'binary'); pdf += (i + 1) + ' 0 obj\n' + objects[i] + '\nendobj\n'; }
const xref = Buffer.byteLength(pdf, 'binary');
pdf += 'xref\n0 ' + (objects.length + 1) + '\n0000000000 65535 f \n';
for (let i = 1; i <= objects.length; i++) pdf += String(offsets[i]).padStart(10, '0') + ' 00000 n \n';
pdf += 'trailer\n<< /Size ' + (objects.length + 1) + ' /Root ' + catalogRef + ' 0 R >>\nstartxref\n' + xref + '\n%%EOF\n';
fs.writeFileSync(output, Buffer.from(pdf, 'binary'));
console.log(`Created ${output} (${pages.length} pages)`);
