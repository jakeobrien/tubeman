import * as firebase from 'firebase';

class Auth {

    constructor() {
        this.user = null;
        this.uid = null;
        this.isLoggedIn = false;
        this.setStateForUser(firebase.auth().currentUser);
        firebase.auth().onAuthStateChanged(this.setStateForUser.bind(this));
    }

    setStateForUser(user) {
        console.log("auth user: " + (user !== null));
        if (user && this.uid === null) {
            this.uid = user.uid;
            this.isLoggedIn = true;
        } else if (user === null && this.uid !== null) {
            this.uid = null;
            this.isLoggedIn = false;
        }
        if (this.stateChanged) this.stateChanged();
    }

    signIn() {
        firebase.auth().signInAnonymously().catch(function (error) {
            console.log(error.code + "  " + error.message);
        });
    }

    signOut() {
        firebase.auth().signOut();
    }

}

export default Auth;