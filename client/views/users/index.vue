<template>
    <div class="tile is-ancestor">
      <div class="tile is-parent">
        <article class="tile is-child box">
          <h4 class="title">Users</h4>
            <a>
                <span class="label" style="color:green" @click="removeMessage">{{message}}</span>
            </a>
          <a>
            <span class="label" style="color:red" @click="removeMessage">{{error}}</span>
          </a>
            <div class="content">
                <a class="button is-warning" @click='view'>
                    Refresh
                </a>
                <a class="button is-info" @click='train'>
                    Train neural network
                </a>
            </div>
          <table class="table">
            <thead>
              <tr>
                <th><abbr title="Position">#</abbr></th>
                <th>Photo</th>
                <th>Name</th>
                <th>Created</th>
                <th></th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(user, index) in users">
                <th>
                 <span>{{index + 1}} </span>
                </th>
                  <td v-if="user.photo != null"><avatar :username="user.name" :size="50" :src="user.photo"></avatar></td>
                  <td v-else><avatar :username="user.name" :size="50"></avatar></td>
                <td>
                  <router-link :to="{ name: 'User', params: { id: user.id }}">{{user.name}}</router-link>
                </td>
                <td>{{user.created}}</td>
                <td>{{user.recognitionId}}</td>
                <td>
                  <a class="button is-danger" @click='deleteUser(user, index)'>
                    Delete user
                  </a>
                </td>
              </tr>
            </tbody>
          </table>
        </article>
      </div>
    </div>
  </div>
</template>

<script>
  import usersDs from 'components/UsersDataService'
  import accountDs from 'components/AccountDataService'
  import Avatar from 'vue-avatar/dist/Avatar' 
  
  export default {
    components: {       
        Avatar
    },
    data() {
      return {
        userName: '',
        users: [],
        message: '',
        error:''

      }
    }, 
    beforeRouteEnter (to, from, next) {
      usersDs.getAll().then((response) => {
        next(vm => {
          vm.users = response.data;
        });
      });
    }, 
    methods: {
      view() {
        usersDs.getAll().then((response) => {
            this.users = response.data;
          });
      },
        train() {
            usersDs.train().then((response) =>{
                if(response.data.statusCode === 200)
                    this.message = "Train neural network successfully completed"
            });
        },
        removeMessage(){
          this.message = '';
          this.error = '';
        },
        deleteUser(user, index) {
          usersDs.deleteUser(user.id, user.personId, user.created, user.name, user.photo, user.isUploadPhoto).then((res) => {
            if (res.data.statusCode === 200) {
              this.message = "User successfully deleted!";
              this.users.splice(index, 1);
            } else {
              this.error = "Error: " + res.data.value;
            }
          })
        }
    }
  }
</script>
