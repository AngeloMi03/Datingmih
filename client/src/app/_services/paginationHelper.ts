import { HttpClient, HttpParams } from "@angular/common/http";
import { PaginatedResult } from "../_models/Pagination";
import { Observable, map, of, take } from 'rxjs';

export function getPaginationResult<T>(url, params, http : HttpClient) {
    const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();
    return http.get<T>(url, { observe: 'response', params }).pipe(
      map((response) => {
        paginatedResult.result = response.body;
        if (response.headers.get('Pagination') !== null) {
          paginatedResult.pagination = JSON.parse(
            response.headers.get('Pagination')
          );
        }
        return paginatedResult;
      })
    );
  }

  export function getPaginationHeader(pageNumber: number, pageSize: number) {
    let params = new HttpParams();

    params = params.append('PageNumber', pageNumber);
    params = params.append('PageSize', pageSize);

    return params;
  }