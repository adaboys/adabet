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

class CreateBonusesTable extends Migration
{
    
    public function up()
    {
        Schema::create('bonuses', function (Blueprint $table) {
            
            $table->bigIncrements('id');
            $table->unsignedBigInteger('account_id');
            $table->unsignedTinyInteger('type');
            $table->decimal('amount', 20, 2);
            $table->timestamps();
            
            $table->foreign('account_id')->references('id')->on('accounts')->onUpdate('cascade')->onDelete('cascade');
        });
    }

    
    public function down()
    {
        Schema::dropIfExists('bonuses');
    }
}
