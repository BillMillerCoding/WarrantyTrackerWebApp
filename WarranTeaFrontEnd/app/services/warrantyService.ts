import type { Warranty, WarrantyUpload, OcrResult } from "~/types";
import { useApiBase } from "~/composables/useApi";

function api() {
  return `${useApiBase()}/warranties`;
}

export async function getWarranties(
  query?: string,
  brand?: string,
  status?: string,
): Promise<Warranty[]> {
  const params: Record<string, string> = {};
  if (query) params.query = query;
  if (brand) params.brand = brand;
  if (status) params.status = status;
  return await $fetch<Warranty[]>(api(), { params, credentials: "include" });
}

export async function getWarrantyById(
  id: number | string,
): Promise<Warranty | undefined> {
  try {
    return await $fetch<Warranty>(`${api()}/${id}`, {
      credentials: "include",
    });
  } catch {
    return undefined;
  }
}

export async function createWarranty(data: WarrantyUpload): Promise<Warranty> {
  const formData = new FormData();
  formData.append("productName", data.productName);
  formData.append("brand", data.brand);
  formData.append("category", data.category);
  formData.append("purchaseDate", data.purchaseDate);
  formData.append("expirationDate", data.expirationDate);
  formData.append("coverageType", data.coverageType);
  if (data.notes) formData.append("notes", data.notes);
  if (data.productId) formData.append("productId", String(data.productId));
  if (data.file) formData.append("file", data.file);
  return await $fetch<Warranty>(api(), {
    method: "POST",
    body: formData,
    credentials: "include",
  });
}

export async function deleteWarranty(id: number | string): Promise<void> {
  await $fetch(`${api()}/${id}`, { method: "DELETE", credentials: "include" });
}

export async function searchWarranties(
  query: string,
  filters?: { brand?: string; status?: string },
): Promise<Warranty[]> {
  return await getWarranties(query, filters?.brand, filters?.status);
}

export async function scanReceipt(file: File): Promise<OcrResult> {
  const formData = new FormData();
  formData.append("file", file);
  return await $fetch<OcrResult>(`${useApiBase()}/ocr/parse-warranty`, {
    method: "POST",
    body: formData,
    credentials: "include",
  });
}

export async function uploadReceipt(
  warrantyId: number | string,
  file: File,
): Promise<Warranty> {
  const formData = new FormData();
  formData.append("file", file);
  return await $fetch<Warranty>(`${api()}/${warrantyId}/upload-receipt`, {
    method: "POST",
    body: formData,
    credentials: "include",
  });
}

export async function getExtractedText(
  warrantyId: number | string,
): Promise<string> {
  return await $fetch<string>(`${api()}/${warrantyId}/extracted-text`, {
    responseType: "text",
    credentials: "include",
  });
}
