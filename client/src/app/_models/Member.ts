import {Photo} from "./Photo";

export interface Member {
    id : Number;
    username : String;
    photoUrl : String;
    age : Number;
    knowAs : String;
    created : Date;
    lastActive : Date;
    gender : String;
    introduction : String;
    lookingfor : String;
    interests : String;
    city : String;
    country : String;
    photos: Photo[];

}