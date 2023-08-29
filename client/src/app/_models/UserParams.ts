import { User } from "./Users";

export class UseParams{
    gender : string;
    minAge = 15;
    maxAge = 140;
    pageNumber = 1;
    pageSize = 5;
    orderBy = "lastActive";

    constructor(user : User){
        this.gender = user.gender == "female"? "male" : "female";
    }

}