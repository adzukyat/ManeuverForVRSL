#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="${ROOT_DIR:-$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)}"
UPSTREAM_REPO="${SLM_UPSTREAM_REPO:-https://github.com/murasaqi/Unity_StageLightManeuver.git}"
UPSTREAM_REF="${SLM_UPSTREAM_REF:-v1.0.2}"
PACKAGE_PATH="${SLM_PACKAGE_PATH:-jp.iridescenet.stagelightmaneuver}"
TARGET_DIR="${SLM_TARGET_DIR:-${ROOT_DIR}/StageLightManeuver}"
PATCH_FILE="${SLM_PATCH_FILE:-${ROOT_DIR}/patches/stage-light-maneuver-vrchat.patch}"

tmp_dir="$(mktemp -d)"
cleanup() {
  rm -rf "${tmp_dir}"
}
trap cleanup EXIT

checkout_dir="${tmp_dir}/Unity_StageLightManeuver"

echo "[vendor-slm] Cloning ${UPSTREAM_REPO}"
git clone --quiet "${UPSTREAM_REPO}" "${checkout_dir}"
git -C "${checkout_dir}" checkout --quiet --detach "${UPSTREAM_REF}"

source_dir="${checkout_dir}/${PACKAGE_PATH}"
if [[ ! -d "${source_dir}" ]]; then
  source_dir=""
  while IFS= read -r manifest; do
    if grep -q '"name"[[:space:]]*:[[:space:]]*"jp.iridescent.stagelightmaneuver"' "${manifest}"; then
      source_dir="$(dirname "${manifest}")"
      break
    fi
  done < <(find "${checkout_dir}" -name package.json -type f)
fi

if [[ -z "${source_dir}" || ! -d "${source_dir}" ]]; then
  echo "[vendor-slm] Could not find Stage Light Maneuver package path." >&2
  exit 1
fi

if [[ ! -f "${checkout_dir}/LICENSE" ]]; then
  echo "[vendor-slm] Upstream LICENSE file was not found." >&2
  exit 1
fi

if [[ ! -f "${PATCH_FILE}" ]]; then
  echo "[vendor-slm] Patch file not found: ${PATCH_FILE}" >&2
  exit 1
fi

echo "[vendor-slm] Rebuilding ${TARGET_DIR}"
rm -rf "${TARGET_DIR}"
mkdir -p "${TARGET_DIR}"
cp -R "${source_dir}/." "${TARGET_DIR}/"
cp "${checkout_dir}/LICENSE" "${TARGET_DIR}/LICENSE"

rm -f "${TARGET_DIR}/package.json" "${TARGET_DIR}/package.json.meta"

cat > "${TARGET_DIR}.meta" <<'META'
fileFormatVersion: 2
guid: 7d774c391994e4495ad88492a09ba54d
folderAsset: yes
DefaultImporter:
  externalObjects: {}
  userData:
  assetBundleName:
  assetBundleVariant:
META

cat > "${TARGET_DIR}/LICENSE.meta" <<'META'
fileFormatVersion: 2
guid: 7a7bb896e4ab44fb91ba6b67cae3e631
DefaultImporter:
  externalObjects: {}
  userData:
  assetBundleName:
  assetBundleVariant:
META

echo "[vendor-slm] Applying ${PATCH_FILE}"
patch -p1 -d "${TARGET_DIR}" < "${PATCH_FILE}"

echo "[vendor-slm] Done: ${TARGET_DIR}"
