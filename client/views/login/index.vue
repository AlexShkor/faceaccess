<template>
  <div>
    <div class="tile is-ancestor">
      <div class="tile is-parent is-12">
        <article class="tile is-child box">
          <h4 class="title">Log in</h4>
          <div class="content">
            <div class="control is-grouped">
              <p class="control is-expanded">
                <input class="input" v-model="Email" type="text" placeholder="Email">
              </p>
              <p class="control is-expanded">
                <input class="input" v-model="Password" type="password" placeholder="Password">
              </p>
              <p class="control">
                <a class="button is-info" @click='sendForm'>
                    Submit
                  </a>
              </p>
            </div>
            <div class="control is-grouped">
              <a class="nav-item hero-brand" href="#/ForgotPassword">
                <div class="is-hidden-mobile">
                  <span>Forgot the password and want to change it</span>
                </div>
              </a>
            </div>
            <div class="control is-grouped">
              <a class="button is-success" href="#/RegistrationAccount">
                <div class="is-hidden-mobile">
                  <span>Registration</span>
                </div>
              </a>
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
        Password:''
      }
    },
    methods: {
      sendForm(){
        accountDs.sendLoginForm(this.Email, this.Password).then((response) => {
        if(response.data === "ADMIN"){
                localStorage.setItem('IsAdmin', true);
        }
        if(response.data === "USER"){
                localStorage.setItem('IsAdmin', false);
        }
        location.reload();
        this.$router.go('Home');
        });
      }
    }
  }
</script>
