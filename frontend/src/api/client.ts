import { API_BASE_URL } from "./config";

type RequestOptions = Omit<RequestInit, "body"> & {
  body?: unknown;
};

export class ApiError extends Error {
  readonly status: number;
  readonly statusText: string;
  readonly details?: unknown;

  constructor(
    message: string,
    status: number,
    statusText: string,
    details?: unknown,
  ) {
    super(message);
    this.name = "ApiError";
    this.status = status;
    this.statusText = statusText;
    this.details = details;
  }
}

export async function gatewayRequest<T>(
  path: string,
  options: RequestOptions = {},
): Promise<T> {
  if (!API_BASE_URL) {
    throw new Error("VITE_API_BASE_URL is not configured.");
  }

  const response = await fetch(`${API_BASE_URL}${path}`, {
    ...options,
    headers: {
      Accept: "application/json",
      ...(options.body ? { "Content-Type": "application/json" } : {}),
      ...options.headers,
    },
    body: options.body ? JSON.stringify(options.body) : undefined,
  });

  if (!response.ok) {
    const details = await readResponseBody(response);
    const message = buildApiErrorMessage(path, response, details);

    throw new ApiError(message, response.status, response.statusText, details);
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return (await response.json()) as T;
}

async function readResponseBody(response: Response): Promise<unknown> {
  const contentType = response.headers.get("content-type") ?? "";

  if (contentType.includes("application/json")) {
    try {
      return await response.json();
    } catch {
      return undefined;
    }
  }

  const text = await response.text();
  return text || undefined;
}

function buildApiErrorMessage(
  path: string,
  response: Response,
  details: unknown,
) {
  const detailText =
    typeof details === "string"
      ? details
      : details && typeof details === "object"
        ? JSON.stringify(details)
        : undefined;

  return [
    `Gateway request failed: ${response.status} ${response.statusText}`,
    `Path: ${path}`,
    detailText ? `Details: ${detailText}` : undefined,
  ]
    .filter(Boolean)
    .join(". ");
}
