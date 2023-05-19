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

class CreateChatRoomsTable extends Migration
{
    
    public function up()
    {
        Schema::create('chat_rooms', function (Blueprint $table) {
            
            $table->bigIncrements('id');
            $table->string('name', 50);
            $table->boolean('enabled')->default(TRUE);
            $table->timestamps();
            
            $table->index('enabled');
        });
    }

    
    public function down()
    {
        Schema::dropIfExists('chat_rooms');
    }
}
