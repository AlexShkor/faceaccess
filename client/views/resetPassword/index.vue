<template>
  <div>
    <div class="tile is-ancestor">
      <div class="tile is-parent is-12">
        <article class="tile is-child box">
          <h4 class="title">Reset password</h4>
            <div v-for="error in Errors">
                <a class="button is-danger" @click='deleteError(error)'>
                    Error: {{error}}
                </a>
                <br></br>
            </div>
          <div class="content">
            <div class="control is-grouped">
              <p class="control is-expanded">
                <input class="input" v-model="Email" type="text" placeholder="Email">
              </p>
              <p class="control is-expanded">
                <input class="input" v-model="Password" type="password" placeholder="Password ">
              </p>
			  <p class="control is-expanded">
                <input class="input" v-model="ConfirmPassword" type="password" placeholder="Confirm Password ">
              </p>
              <p class="control">
                <a class="button is-info" @click='sendForm'>
                    Submit
                  </a>
              </p>
            </div>                       
          </div>
        </article>
      </div>
    </div>
  </div>
</template>

<script>
  import accountDs from 'components/AccountDataService'

  export default {
    data() {
      return {
        Email:'',
        Password:'',
        ConfirmPassword:'',
        Errors:[]
      }
    },
    methods: {
      sendForm(){
       if(this.Password === this.ConfirmPassword){
         accountDs.sendResetPasswordForm(this.Email, this.Password, this.ConfirmPassword, this.$route.params[0]).then((response)=> {
             if(response.data.statusCode === 400){
                 if(Object.prototype.toString.call(response.data.value) === '[object Array]' ) {
                     for (var i = 0; i < response.data.value.length; i++){
                         this.Errors.push(response.data.value[i].description);
                     }
                 }else{
                     this.Errors.push(response.data.value);
                 }
             }
            });
          } else{
             this.Errors.push("Password and confirm password don't equal");
          }
      },
      deleteError(data){
          var index = this.Errors.indexOf(data);
          this.Errors.splice(index, 1);
      }
    }
  }
</script>
