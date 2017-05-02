<template>
  <div>
    <div class="tile is-ancestor">
      <div class="tile is-parent is-12">
        <article class="tile is-child box">
          <h4 class="title">Registration Account</h4>
          <div v-if="Error != ''">
            <a class="button is-danger" @click='deleteError'>
              Error: {{Error}}
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
    Error:''
      }
    },
    methods: {
      sendForm(){
        if(this.Password === this.ConfirmPassword){
           accountDs.sendRegisterForm(this.Email, this.Password, this.ConfirmPassword).then((response) => {
              if(response.data.statusCode === 400){
                 this.Error = response.data.value;
              }
            });
          } else{
             this.Error = "Password and confirm password don't equal"
          }
      },
      deleteError(){
          this.Error = '';
      }
    }
  }
</script>
