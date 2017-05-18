<template>
  <div>
    <div class="tile is-ancestor">
      <div class="tile is-parent is-12">
        <article class="tile is-child box">
          <h4 class="title">Forgot password</h4>
		      <h3>Follow the link in the email that will be sent to you</h3>
          <br/>
            <div v-for="error in Errors">
                <a class="button is-danger" @click='deleteError(error)'>
                    Error: {{error}}
                </a>
                <br></br>
            </div>
          <div v-if="MessageOk">
            <a class="button is-success" @click='deleteMessageOk()'>
              {{MessageOk}}
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
        Errors: [],
        MessageOk:''
      }
    },
    methods: {
      sendForm(){
        accountDs.forgotPassword(this.Email).then((response) =>{
            if(response.data.statusCode === 400){
                if(Object.prototype.toString.call(response.data.value) === '[object Array]' ) {
                    for (var i = 0; i < response.data.value.length; i++){
                        this.Errors.push(response.data.value[i].description);
                    }
                }else{
                    this.Errors.push(response.data.value);
                }
            } else {
              this.MessageOk = 'The message was sent successfully to your email address!'
              this.Email = '';
            }
        });
      },
      deleteError(data){
          var index = this.Errors.indexOf(data);
          this.Errors.splice(index, 1);
      },
      deleteMessageOk() {
        this.MessageOk = '';
      }
    }
  }
</script>