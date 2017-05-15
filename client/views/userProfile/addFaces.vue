<template>
    <div>
        <div class="tile is-ancestor">
            <div class="tile is-parent is-3">
                <article class="tile is-child box">
                    <h4 class="title"></h4>
                    <div class="content">
                        <vue-webcam ref='webcam'></vue-webcam>
                    </div>
                    <div class="block">
                        <button class="button is-primary" @click='takePhoto'>Take Photo</button>
                    </div>
                </article>
            </div>
            <div class="tile is-parent is-9">
                <article class="tile is-child box">
                    <h4 class="title">Add Faces</h4>
                    <div class="columns is-multiline">
                        <div class="content" v-for="(photo, index) in currentPhotos">
                            <div style="padding:5px">
                                <div>
                                    <a class="level-item">
                                        <span class="icon is-small">#{{index +1}} </span>
                                    </a>
                                    <nav class="level">
                                        <div class="level-left">
                                            <a class="level-item" title="Delete" @click='deleteCurrenyFace(index)'>
                                                <span class="icon is-small"><i class="fa fa-trash"></i></span>
                                            </a>
                                        </div>
                                    </nav>
                                    <div></div>
                                </div>
                                <img id='img' class="box" :src="photo" alt="" style="width:230px; height:auto; border:solid; color:green; padding:8px" />
                            </div>
                        </div>
                    </div>
                    <div class="block">
                        <button class="button is-success" @click='upload' :disabled="!currentPhotos.length">Upload Current Photos</button>
                    </div>
                </article>
            </div>

        </div>
        <div class="columns is-multiline">
            <div v-for='(photo, index) in photos' class="column is-one-quarter">
                <div>
                    <a class="level-item">
                        <span class="icon is-small">#{{index +1}} </span>
                    </a>
                    <nav class="level">
                        <div class="level-left">
                            <a class="level-item" title="Delete" @click='deleteFace(index, photo.id)'>
                                <span class="icon is-small"><i class="fa fa-trash"></i></span>
                            </a>
                        </div>
                    </nav>
                    <div></div>
                </div>
                <img class="box" :title="photo.id" :src='photo.img || photo.imageBase64' />
            </div>
        </div>
    </div>
</template>

<script>
  import usersDs from 'components/UsersDataService'
  import VueWebcam from 'components/Webcam'

  export default {
    components: {
        VueWebcam
    },
    data() {
        return {
            currentPhotos: [],
            currentPhoto: null,
            photos: []
        }
    },
    mounted(){
        usersDs.getFaces(this.$route.params.id).then((res)=>{
            this.photos = res.data;
        })
    },
    methods: {
        takePhoto() {
            this.currentPhoto = this.$refs.webcam.getPhoto();
            this.currentPhotos.push(this.currentPhoto);
        },
      upload() {
          var userFaces = [];
          this.currentPhotos.forEach(function(photo){
              userFaces.push(photo.toString('base64'))
          });
          usersDs.addFace(this.$route.params.id, userFaces).then((res) => {
              this.photos = this.photos.concat(res.data);
              this.currentPhotos = [];
          });
          this.currentPhoto = null;
      },
      deleteFace(index ,photoId){
          usersDs.deletePhoto(photoId).then((response) => {
              if(response.data.statusCode === 200){
                  this.photos.splice(index, 1);
              }
          });
      },
        deleteCurrenyFace(index){
            this.currentPhotos.splice(index, 1);
        }
    }
    }
</script>