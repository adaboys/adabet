import { sportTypes } from '@constants';

const sportImages = {
    [sportTypes.SOCCER]: '/images/sports/football.svg',
    [sportTypes.TENNIS]: '/images/sports/tennis.png'
}

const TeamImage = ({ id, sportType, size = 's', className }) => {
    return <img
        src={id ? `https://assets.b365api.com/images/team/${size}/${id}.png` : sportImages[sportType]}
        className={className}
     />
}

export default TeamImage;
