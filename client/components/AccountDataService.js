import DataService from './DataService'

export default {
  sendLoginForm(email, password){
      return DataService.post("account/login", {Email:email, Password:password});
  },
  sendRegisterForm(email, password, confirmPassword){
      return DataService.post("account/register", {Email:email, Password:password, ConfirmPassword:confirmPassword});
  },
  forgotPassword(email){
      return DataService.post("account/forgotPassword", {Email:email});
  },
  sendResetPasswordForm(email, password, confirmPassword, code){
      return DataService.post("account/resetPassword", {Email:email, Password:password, ConfirmPassword:confirmPassword, Code:code})
  },
  logOff(){
      return DataService.post("account/logOff",{})
  }
}
