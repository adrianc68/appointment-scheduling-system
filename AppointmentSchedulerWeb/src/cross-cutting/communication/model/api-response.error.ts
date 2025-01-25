export interface ApiResponseError<TData> {
  status: number;
  message: string;
  data: TData,
  version: string;
}
