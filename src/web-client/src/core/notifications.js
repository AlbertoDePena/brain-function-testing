import * as Toastr from 'toastr/toastr';

Toastr.options.preventDuplicates = true;
Toastr.options.progressBar = true;
Toastr.options.positionClass = 'toast-bottom-center';

export function notifyError(message) {
  Toastr.error(message);
}

export function notifySuccess(message) {
  Toastr.success(message);
}
