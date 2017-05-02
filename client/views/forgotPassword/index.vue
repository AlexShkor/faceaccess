<template>
  <div>
    <div class="tile is-ancestor">
      <div class="tile is-parent is-12">
        <article class="tile is-child box">
          <h4 class="title">Forgot password</h4>
		      <h3>Follow the link in the email that will be sent to you</h3>
          <br/>
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
        Error:''
      }
    },
    methods: {
      sendForm(){
        accountDs.forgotPassword(this.Email).then((response) =>{
         if(response.data.statusCode === 400){
            this.Error = response.data.value;
          }
        });
      },
      deleteError(){
        this.Error = '';
      }
    }
  }
</script>