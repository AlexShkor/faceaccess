<template>
    <div>
        <div class="tile is-ancestor">
            <div class="tile is-parent is-6">
                <article class="tile is-child box" v-if="image != ''">
                    <div>
                        <div >
                            <h2 class="label">You Avatar:</h2>
                            <a>
                                <span class="label" style="color:green" @click="removeMessage">{{message}}</span>
                            </a>
                            <img :src="image" style="width:auto;height:300px;border:ridge" />
                        </div>
                            <div v-if="image == 'https://www.gravatar.com/avatar/00000000000000000000000000000000?d=mm&f=y'">
                                <a>
                                    <span class="fileContainer">
                                        Update avatar!
                                        <input type="file" @change="onFileChange">
                                    </span>
                                </a>
                            </div>
                            <div v-else>
                                <button class="button is-danger" @click="removeImage">Remove image</button>
                                <button class="button is-success" @click='upload'>Upload Current</button>
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
            image: '',
            message:''
      }
    },
     mounted(){
         accountDs.getUserAvatar(this.$route.params.id).then((res)=>{
             if(res.data.statusCode != 400){
                 this.image = res.data;
             }else{
                 this.image ="https://www.gravatar.com/avatar/00000000000000000000000000000000?d=mm&f=y"
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
            };
            reader.readAsDataURL(file);
        },
        removeImage(){
            accountDs.deleteUserAvatar(this.$route.params.id, '').then((res) => {
                if(res.data.statusCode === 200){
                    this.image = 'https://www.gravatar.com/avatar/00000000000000000000000000000000?d=mm&f=y';
                }
            })       
        },
        upload(){
            accountDs.addUserAvatar(this.$route.params.id, this.image.toString('base64')).then((res) => {
                if(res.data.statusCode === 200){
                    this.message = "Avatar successfully loaded"
                }
            })
        },
        removeMessage(){
            this.message = '';
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
</style>
