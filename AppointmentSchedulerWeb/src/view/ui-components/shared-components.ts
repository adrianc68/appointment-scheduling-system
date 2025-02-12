import { GridListComponent } from "./display/grid-list/grid-list.component";
import { TextInputComponent } from "./input/text-input/text-input.component";
import { DropdownComponent } from "./navigation/dropdown/dropdown.component";
import { ButtonTaskComponent } from "./others/button-task/button-task.component";
import { ButtonComponent } from "./others/button/button.component";
import { ErrorValidationMessageFormComponent } from "./utils/error-validation-message-form/error-validation-message-form.component";

export const SHARED_STANDALONE_COMPONENTS = [
  ButtonTaskComponent,
  ButtonComponent,
  TextInputComponent,
  ErrorValidationMessageFormComponent,
  DropdownComponent,
  GridListComponent
]

