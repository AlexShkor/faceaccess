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
 
getUser(userId) {
    return DataService.post("account/getUser", {UserId:userId});
},
getAvatarDefault() {
    return DataService.post("account/getAvatarDefault", {});
},
changePasswordFromUserProfile(email, newPassword) {
    return DataService.post("account/changePasswordFromUserProfile", {Email:email, NewPassword:newPassword});
},
updateUser(id, photo, personId, fullName, created, isUploadPhoto) {
    return DataService.post("account/updateUser", { Id: id, Photo: photo, PersonId: personId, Name: fullName, Created: created, IsUploadPhoto: isUploadPhoto });
}
}
