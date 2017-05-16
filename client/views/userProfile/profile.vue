<template>
    <div>
       
        <a :class="{'button is-info' : !isAvatarPage, 'button is-default': isAvatarPage}" @click="getAvatar">Avatar</a>
        <a :class="{'button is-info' : !isDetecFacePage, 'button is-default': isDetecFacePage}" @click="getisDetecFace">Detect face</a>
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
                                    <span class="label" style="color:red" @click="removeMessage">{{errorUploadImg}}</span>                        
                                </a>
                                <img id="img" :src="image" style="width:auto;height:300px;border:ridge" />
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
            <div class="tile is-parent is-6" v-if="isDetecFacePage">
                <article class="tile is-child box" id="outer">
                    <div id="inner">
                        <article class="tile is-child box">
                            <h4 class="title">Detect Face</h4>
                            <div class="content">
                                <div>Пользователь {{ $route.params.id }}</div>
                                <div>Имя {{ currentName }}</div>
                                <div>currentFaceId {{ currentFaceId }}</div>
                                <canvas id="canvas" width="400" height="300"></canvas>
                                <div class="content">
                                    <vue-webcam ref='webcam'></vue-webcam>
                                </div>
                                <div class="block">
                                    <button class="button is-primary" @click='takePhoto'>Take Photo</button>
                                    <button class="button is-danger" @click='detect'>Detect Face</button>
                                </div>
                            </div>
                        </article>
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
  import usersDs from 'components/UsersDataService'
  import VueWebcam from 'components/Webcam'

  var interval = null;
  const prefix = "data:image/jpeg;base64,";

  export default {
    components: {
        VueWebcam
    },
    data() {
        return {
            image: '',
            message:'',
            isSelectedPhotos: false,

            isAvatarPage: true,
            isDetecFacePage: false,
            isPasswordPage: false,

            email:'',
            password:'',
            confirmPassword:'',
            errorsChangePassword:[],
            messageOkChangePassword:'',

            fullName:'',
            personId:'',
            created:'',
            errorUploadImg: '',

            currentPhoto: null,
            currentFaceId: null,
            currentName: null,
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
        takePhoto() {
            this.currentName = '';
            this.currentPhoto = this.$refs.webcam.getPhoto();
            var canvas = document.getElementById('canvas');
            var ctx = canvas.getContext('2d');
            var img = new Image
            img.onload = function() {
                ctx.drawImage(img, 0, 0); // Or at whatever offset you like
            };
            img.src = this.currentPhoto;
        },
      detect() {
          if(this.currentPhoto == null){
              return;
          }
          usersDs.detectFace(this.$route.params.id, this.currentPhoto.toString('base64')).then((res) => {
              console.log(res)
              var rect = res.data.rect;
              this.currentFaceId = res.data.faceId;
              var canvas = document.getElementById('canvas');
              var ctx = canvas.getContext('2d');
              ctx.beginPath();
              ctx.lineWidth = "2";
              ctx.strokeStyle = "green";
              ctx.rect(rect.left, rect.top, rect.width, rect.height)
              ctx.stroke();
              for (let key in res.data.landmarks) {
                  let point = res.data.landmarks[key];
                  console.log(point);
                  ctx.beginPath();
                  ctx.lineWidth = "2";
                  ctx.strokeStyle = "blue";
                  ctx.rect(point.x, point.y, 1, 1)
                  ctx.stroke();
              };
              this.currentName = '';
              usersDs.identify(this.currentFaceId).then((res) => {
                  this.currentName = res.data.name;
              });
          })
      },
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
            if(!file.type.match(/image.*/)){
                return this.errorUploadImg ="This file not image!"
            }
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
            var img = document.getElementById("img");    
            img.src = this.image;

            var canvas = document.createElement("canvas")
            canvas.width = img.width;
            canvas.height = img.height;
            var ctx = canvas.getContext("2d");
            ctx.drawImage(img, 0, 0, img.width, img.height);
            this.image = canvas.toDataURL("image/jpeg");           

            accountDs.updateUser(this.$route.params.id, this.isSelectedPhotos ? this.image.toString('base64') : null, this.personId, this.fullName, this.created, this.isSelectedPhotos).then((res) => {
                if(res.data.statusCode === 200){
                    this.message = "Changes successfully saved";
                }
            })
        },
        removeMessage(){
            this.message = '';
            this.errorUploadImg = '';
        },
         removeMessageOkChangePassword(){
             this.messageOkChangePassword = '';
        },
        getAvatar(){
            this.isAvatarPage = true;
            this.isDetecFacePage = false;
            this.isPasswordPage = false;
        },
        getPassword(){
            this.isAvatarPage = false;
            this.isDetecFacePage = false;
            this.isPasswordPage = true;
        },       
        getisDetecFace(){
            this.isAvatarPage = false;
            this.isDetecFacePage = true;
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
