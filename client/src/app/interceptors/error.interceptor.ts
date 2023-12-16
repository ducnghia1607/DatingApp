import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse,
} from '@angular/common/http';
import { Observable, catchError } from 'rxjs';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(private router: Router, public toastService: ToastrService) {}

  intercept(
    request: HttpRequest<unknown>,
    next: HttpHandler
  ): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        {
          if (error) {
            switch (error.status) {
              case 400:
                if (error.error.errors) {
                  const modelStatusCode = [];
                  for (var key in error.error.errors) {
                    if (error.error.errors[key]) {
                      modelStatusCode.push(error.error.errors[key]);
                    }
                  }
                  throw modelStatusCode.flat;
                } else {
                  this.toastService.error(error.error, error.status.toString());
                }
                break;

              case 401:
                this.toastService.error(
                  'Unauthorised',
                  error.status.toString()
                );
                break;
              case 404:
                this.router.navigateByUrl('/not-found');
                break;
              case 500:
                const navigationExtras: NavigationExtras = {
                  state: { error: error.error },
                };
                this.router.navigateByUrl('/server-error', navigationExtras);
                break;
              default:
                this.toastService.error('Something unexpected went wrong');
                console.log(error);
                break;
            }
          }

          throw error;
        }
      })
    );
  }
}
