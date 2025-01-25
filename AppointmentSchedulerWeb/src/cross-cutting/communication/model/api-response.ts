export interface ApiResponse<TData, TError> {
  status: number;
  message: string;
  data: TData | TError,
  version: string;
}
