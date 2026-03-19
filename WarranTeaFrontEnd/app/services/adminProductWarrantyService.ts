import type { ProductWarranty } from "~/types";

const API = "/api/admin/product-warranties";

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
  return await $fetch<ProductWarranty[]>(API);
}

export async function getProductWarrantyById(
  id: number,
): Promise<ProductWarranty | undefined> {
  try {
    return await $fetch<ProductWarranty>(`${API}/${id}`);
  } catch {
    return undefined;
  }
}

export async function createProductWarranty(
  data: CreateProductWarrantyInput,
): Promise<ProductWarranty> {
  return await $fetch<ProductWarranty>(API, { method: "POST", body: data });
}

export async function updateProductWarranty(
  id: number,
  data: UpdateProductWarrantyInput,
): Promise<ProductWarranty> {
  return await $fetch<ProductWarranty>(`${API}/${id}`, {
    method: "PUT",
    body: data,
  });
}

export async function deleteProductWarranty(id: number): Promise<void> {
  await $fetch(`${API}/${id}`, { method: "DELETE" });
}

export async function reindexProductWarranties(): Promise<{ message: string }> {
  return await $fetch<{ message: string }>(`${API}/reindex`, {
    method: "POST",
  });
}
