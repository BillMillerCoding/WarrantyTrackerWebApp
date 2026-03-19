import type { ProductWarranty } from "~/types";
import { useApiBase } from "~/composables/useApi";

function api() {
  return `${useApiBase()}/admin/product-warranties`;
}

export interface CreateProductWarrantyInput {
  productId: number;
  coverageType: string;
  durationMonths: number;
  provider: string;
  description?: string;
  url?: string;
}

export interface UpdateProductWarrantyInput {
  coverageType?: string;
  durationMonths?: number;
  provider?: string;
  description?: string;
  url?: string;
}

export async function getProductWarranties(): Promise<ProductWarranty[]> {
  return await $fetch<ProductWarranty[]>(api(), { credentials: "include" });
}

export async function getProductWarrantyById(
  id: number,
): Promise<ProductWarranty | undefined> {
  try {
    return await $fetch<ProductWarranty>(`${api()}/${id}`, {
      credentials: "include",
    });
  } catch {
    return undefined;
  }
}

export async function createProductWarranty(
  data: CreateProductWarrantyInput,
): Promise<ProductWarranty> {
  return await $fetch<ProductWarranty>(api(), {
    method: "POST",
    body: data,
    credentials: "include",
  });
}

export async function updateProductWarranty(
  id: number,
  data: UpdateProductWarrantyInput,
): Promise<ProductWarranty> {
  return await $fetch<ProductWarranty>(`${api()}/${id}`, {
    method: "PUT",
    body: data,
    credentials: "include",
  });
}

export async function deleteProductWarranty(id: number): Promise<void> {
  await $fetch(`${api()}/${id}`, { method: "DELETE", credentials: "include" });
}

export async function reindexProductWarranties(): Promise<{ message: string }> {
  return await $fetch<{ message: string }>(`${api()}/reindex`, {
    method: "POST",
    credentials: "include",
  });
}
