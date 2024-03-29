<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *   database/migrations
 * 
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
*/

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

class CreateGameRoomPlayersTable extends Migration
{
    
    public function up()
    {
        Schema::create('game_room_players', function (Blueprint $table) {
            
            $table->id();
            $table->foreignId('game_room_id');
            $table->foreignId('user_id');
            $table->foreignId('game_id')->nullable();
            $table->timestamps();
            
            $table->foreign('game_room_id')->references('id')->on('game_rooms')->onUpdate('cascade')->onDelete('cascade');
            $table->foreign('user_id')->references('id')->on('users')->onUpdate('cascade')->onDelete('cascade');
            $table->foreign('game_id')->references('id')->on('games')->onUpdate('cascade')->onDelete('cascade');
            
            $table->unique(['game_room_id', 'user_id']);
        });
    }

    
    public function down()
    {
        Schema::dropIfExists('game_room_players');
    }
}
