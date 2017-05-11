<template>
    <div>
       
        <a :class="{'button is-info' : !isAvatarPage, 'button is-default': isAvatarPage}" @click="getAvatar">Avatar</a>
        <a :class="{'button is-info' : !isRecognitionLogsPage, 'button is-default': isRecognitionLogsPage}" @click="getRecognitionLogs">Recognition logs</a>
        <a :class="{'button is-info' : !isPasswordPage, 'button is-default': isPasswordPage}" @click="getPassword">Change password</a>

        <div class="tile is-ancestor">
            <div class="tile is-parent is-6" v-if="isAvatarPage">
                <article class="tile is-child box" v-if="image != ''" id="outer">
                    <div id="inner">
                        <div>
                            <div>
                                <h2 class="label">You Avatar:</h2>
                                <a>
                                    <span class="label" style="color:green" @click="removeMessage">{{message}}</span>
                                </a>
                                <img :src="image" style="width:auto;height:300px;border:ridge" />
                            </div>
                            <div v-if="isSelectedPhotos == false">
                                <a>
                                    <span class="fileContainer">
                                        Update avatar!
                                        <input type="file" @change="onFileChange">
                                    </span>
                                </a>
                            </div>
                            <div v-else>
                                <button class="button is-danger" @click="removeImage">Remove image</button>                           
                            </div>
                        </div><br /><br />

                        <div>
                            <h2 class="label">Full name:</h2>
                            <div>
                                <p class="control is-expanded">
                                    <input class="input" v-model="fullName" type="text" placeholder="Full name">
                                </p>
                                <p class="control">
                                    <a class="button is-success" @click='updateUser'>
                                        Save changes
                                    </a>
                                </p>
                            </div>
                        </div> 
                    </div>
                </article>
            </div>
            <div class="tile is-parent is-6" v-if="isRecognitionSetupPage">
                <article class="tile is-child box" id="outer">
                    <div id="inner">
                        <h2>Recognition Setup</h2>
                    </div>
                </article>
            </div>
            <div class="tile is-parent is-6" v-if="isRecognitionLogsPage">
                <article class="tile is-child box" id="outer">
                    <div id="inner">
                        <h2>Recognition Logs</h2>
                    </div>
                </article>
            </div>
            <div class="tile is-parent is-6" v-if="isPasswordPage">
                <article class="tile is-child box">
                   <div>   
                       <div v-for="error in errorsChangePassword">
                           <a class="button is-danger" @click='deleteError(error)'>
                               Error: {{error}}
                           </a>
                           <br></br>
                       </div>               
                       <div>
                           <a>
                               <span class="label" style="color:green" @click="removeMessageOkChangePassword">{{messageOkChangePassword}}</span>
                           </a>
                           <p class="control is-expanded">
                               <input class="input" v-model="email" type="text" placeholder="Email">
                           </p>
                           <p class="control is-expanded">
                               <input class="input" v-model="password" type="password" placeholder="Password ">
                           </p>
                           <p class="control is-expanded">
                               <input class="input" v-model="confirmPassword" type="password" placeholder="Confirm Password ">
                           </p>
                           <p class="control">
                               <a class="button is-success" @click='sendFormChangePassword'>
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
  import Avatar from 'vue-avatar/dist/Avatar' 

  export default {
    components: {       
        Avatar
    },
    data() {
        return {
            image: '',
            message:'',
            isSelectedPhotos: false,

            isAvatarPage: true,
            isRecognitionLogsPage: false,
            isPasswordPage: false,

            email:'',
            password:'',
            confirmPassword:'',
            errorsChangePassword:[],
            messageOkChangePassword:'',

            fullName:'',
            personId:'',
            created:''
      }
    },
     mounted(){
         accountDs.getUser(this.$route.params.id).then((res)=>{
             if(res.data.statusCode != 400){
                 this.image = res.data.photo;
                 this.fullName = res.data.name;
                 this.personId = res.data.personId;
                 this.created = res.data.created;               
                 this.isSelectedPhotos = res.data.isUploadPhoto;
             }                    
        })
    },
    methods: {
        onFileChange(e) {
            var files = e.target.files || e.dataTransfer.files;
            if (!files.length)
                return;
            this.createImage(files[0]);
        },
        createImage(file) {
            var image = new Image();
            var reader = new FileReader();
            var vm = this;

            reader.onload = (e) => {
                this.image = e.target.result;
                this.isSelectedPhotos = true;
              
            };
            reader.readAsDataURL(file);
        },
           sendFormChangePassword(){
            if(this.password === this.confirmPassword){
                accountDs.changePasswordFromUserProfile(this.email, this.password).then((response)=> {
                    if(response.data.statusCode === 400){
                        if(Object.prototype.toString.call(response.data.value) === '[object Array]' ) {
                            for (var i = 0; i < response.data.value.length; i++){
                                this.errorsChangePassword.push(response.data.value[i].description);
                            }
                        }else{
                            this.errorsChangePassword.push(response.data.value);
                        }
                    }else{
                        this.email = "";
                        this.password = "";
                        this.confirmPassword = "";
                        this.messageOkChangePassword = "Password successfully changed"
                    }
                });
            } else{
                this.errorsChangePassword.push("Password and confirm password don't equal");
            }
        },
        removeImage(){
            accountDs.updateUser(this.$route.params.id, null, this.personId, this.fullName, this.created, false).then((res) => {
                if(res.data.statusCode === 200){
                    accountDs.getAvatarDefault().then((res) => {
                        this.image = res.data;
                        this.isSelectedPhotos = false;
                    })
                }
            })       
        },
        updateUser(){
            accountDs.updateUser(this.$route.params.id, this.isSelectedPhotos ? this.image.toString('base64') : null, this.personId, this.fullName, this.created, this.isSelectedPhotos).then((res) => {
                if(res.data.statusCode === 200){
                    this.message = "Changes successfully saved";
                }
            })
        },
        removeMessage(){
            this.message = '';
        },
         removeMessageOkChangePassword(){
             this.messageOkChangePassword = '';
        },
        getAvatar(){
            this.isAvatarPage = true;
            this.isRecognitionLogsPage = false;
            this.isPasswordPage = false;
        },
        getPassword(){
            this.isAvatarPage = false;
            this.isRecognitionLogsPage = false;
            this.isPasswordPage = true;
        },       
        getRecognitionLogs(){
            this.isAvatarPage = false;
            this.isRecognitionLogsPage = true;
            this.isPasswordPage = false;
        },
        deleteError(data){
            var index = this.errorsChangePassword.indexOf(data);
            this.errorsChangePassword.splice(index, 1);
        }
  }    
    }
</script>
<style>
    .fileContainer {
    overflow: hidden;
    position: relative;
}

.fileContainer [type=file] {
    cursor: inherit;
    display: block;
    font-size: 999px;
    filter: alpha(opacity=0);
    min-height: 100%;
    min-width: 100%;
    opacity: 0;
    position: absolute;
    right: 0;
    text-align: right;
    top: 0;
}
.fileContainer {
    background: lightblue;
    border-radius: .5em;
    float: left;
    padding: .5em;
}
#inner {
  display: table;
  margin: 0 auto;
}
#outer{
    width: 100%
}
</style>
