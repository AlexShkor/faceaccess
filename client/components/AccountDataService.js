import DataService from './DataService'

export default {
  sendLoginForm(email, password){
      return DataService.post("account/login", {Email:email, Password:password});
  },
  sendRegisterForm(email, fullName, password, confirmPassword){
      return DataService.post("account/register", {Email:email, FullName:fullName, Password:password, ConfirmPassword:confirmPassword});
  },
  forgotPassword(email){
      return DataService.post("account/forgotPassword", {Email:email});
  },
  sendResetPasswordForm(email, password, confirmPassword, code){
      return DataService.post("account/resetPassword", {Email:email, Password:password, ConfirmPassword:confirmPassword, Code:code})
  },
  logOff(){
      return DataService.post("account/logOff",{})
  },
  addUserAvatar(userId, photo) {
      return DataService.post("account/addUserAvatar", {USerId:userId, Photo:photo});
  },
deleteUserAvatar(userId, photo) {
      return DataService.post("account/deleteUserAvatar", {USerId:userId, Photo:photo});
},
getUserAvatar(userId) {
    return DataService.post("account/getUserAvatar", {UserId:userId});
},
getAvatarDefault() {
    return DataService.post("account/getAvatarDefault", {});
}
}
