<template>
    <div class="tile is-ancestor">
      <div class="tile is-parent">
        <article class="tile is-child box">
          <h4 class="title">Users</h4>
            <div class="content">
                <a class="button is-warning" @click='view'>
                    Refresh
                </a>
            </div>
          <table class="table">
            <thead>
              <tr>
                <th><abbr title="Position">#</abbr></th>
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
                <td>
                  <router-link :to="{ name: 'User', params: { id: user.id }}">{{user.name}}</router-link>
                </td>
                <td>{{user.created}}</td>
                <td>{{user.recognitionId}}</td>

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
  
  export default {
    data() {
      return {
        userName: '',
        users: []
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
      }
    }
  }
</script>
