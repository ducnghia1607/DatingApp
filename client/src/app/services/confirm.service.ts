import { Injectable } from '@angular/core';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { Observable, config, map } from 'rxjs';
import { ConfirmDialogComponent } from '../modals/confirm-dialog/confirm-dialog.component';

@Injectable({
  providedIn: 'root',
})
export class ConfirmService {
  modalRef?: BsModalRef;
  constructor(private modalService: BsModalService) {}

  confirm(
    title = 'Confirmation',
    message = 'Are you sure you want to do this',
    btnOkText = 'OK',
    btnCancelText = 'Cancel'
  ): Observable<boolean> {
    const config: ModalOptions = {
      initialState: {
        title: title,
        message: message,
        btnOkText: btnOkText,
        btnCancelText: btnCancelText,
      },
    };

    this.modalRef = this.modalService.show(ConfirmDialogComponent, config);

    return this.modalRef.onHidden!.pipe(
      map(() => {
        return this.modalRef!.content!.result;
      })
    );
  }
}
