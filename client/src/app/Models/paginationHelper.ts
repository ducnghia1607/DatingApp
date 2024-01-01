import { HttpClient, HttpParams } from '@angular/common/http';
import { PaginatedResult } from './pagination';
import { map } from 'rxjs';

export function getPaginatedResult<T>(
  params: HttpParams,
  url: string,
  http: HttpClient
) {
  const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();
  return http
    .get<T>(url, {
      observe: 'response',
      params,
    })
    .pipe(
      map((response) => {
        if (response.body) {
          paginatedResult.result = response.body;
        }
        const pagination = response.headers.get('Pagination');
        if (pagination) {
          paginatedResult.pagination = JSON.parse(pagination);
        }

        return paginatedResult;
      })
    );
}

export function getPaginationHeader(
  pageNumber: number | undefined,
  itemsPerPage: number | undefined
) {
  var params = new HttpParams();
  if (pageNumber && itemsPerPage) {
    params = params.append('pageNumber', pageNumber);
    params = params.append('pageSize', itemsPerPage);
  }
  return params;
}
